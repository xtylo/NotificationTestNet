using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unit.Services
{
    public class NotificationDispatcherServiceTests
    {
        private readonly Mock<IUserRepository>
            _userRepositoryMock = new();

        private readonly Mock<INotificationLogRepository>
            _logRepositoryMock = new();

        private readonly Mock<INotificationChannel>
            _channelMock = new();

        [Fact]
        public async Task DispatchAsync_Should_Send_Notifications_One_Channel()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Eduardo",
                Email = "eduardo@test.com",

                Channels = new List<Channel>
            {
                new Channel
                {
                    Id = (int)NotificationChannelType.Email,
                    Name = "Email",
                    ChannelType = NotificationChannelType.Email
                }
            }
            };

            var message = new Message
            {
                Id = 1,
                Body = "Test notification",
                CategoryId = 1,
                CorrelationId = Guid.NewGuid()
            };

            _userRepositoryMock
                .Setup(x => x.GetSubscribedUsersAsync(
                    message.CategoryId))
                .ReturnsAsync(new List<User> { user });

            _channelMock
                .Setup(x => x.ChannelType)
                .Returns(NotificationChannelType.Email);

                
            _channelMock
                .Setup(x => x.SendAsync(user, message))
                .ReturnsAsync(NotificationResult.Successful());

            var service = new NotificationDispatcherService(
                _userRepositoryMock.Object,
                _logRepositoryMock.Object,
                new List<INotificationChannel>
                {
                _channelMock.Object
                });

            // Act
            await service.DispatchAsync(message);

            // Assert
            _channelMock.Verify(
                x => x.SendAsync(user, message),
                Times.Once);

            _logRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<NotificationLog>()),
                Times.Once);
        }

        [Fact]
        public async Task DispatchAsync_Should_Send_Notifications_Three_Channel()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Eduardo",
                Email = "eduardo@test.com",

                Channels = new List<Channel>
            {
                new Channel
                {
                    Id = (int)NotificationChannelType.Email,
                    Name = "Email",
                    ChannelType = NotificationChannelType.Email
                },
                new Channel
                {
                    Id = (int)NotificationChannelType.Sms,
                    Name = "SMS",
                    ChannelType = NotificationChannelType.Sms
                },
                new Channel
                {
                    Id = (int)NotificationChannelType.Push,
                    Name = "Push",
                    ChannelType = NotificationChannelType.Push 
                }
            }
            };

            var message = new Message
            {
                Id = 1,
                Body = "Test notification",
                CategoryId = 1,
                CorrelationId = Guid.NewGuid()
            };

            _userRepositoryMock
                .Setup(x => x.GetSubscribedUsersAsync(
                    message.CategoryId))
                .ReturnsAsync(new List<User> { user });

            //channel mocks
            Mock<INotificationChannel> emailChannelMock = new();
            emailChannelMock
                .Setup(x => x.ChannelType)
                .Returns(NotificationChannelType.Email);

            emailChannelMock
                .Setup(x => x.SendAsync(user, message))
                .ReturnsAsync(NotificationResult.Successful());

            Mock<INotificationChannel> smsChannelMock = new();
            smsChannelMock
                .Setup(x => x.ChannelType)
                .Returns(NotificationChannelType.Sms);
            smsChannelMock
                .Setup(x => x.SendAsync(user, message))
                .ReturnsAsync(NotificationResult.Successful());

            Mock<INotificationChannel> pushChannelMock = new();
            pushChannelMock
                .Setup(x => x.ChannelType)
                .Returns(NotificationChannelType.Push);
            pushChannelMock 
                .Setup(x => x.SendAsync(user, message))
                .ReturnsAsync(NotificationResult.Successful());

            var service = new NotificationDispatcherService(
                _userRepositoryMock.Object,
                _logRepositoryMock.Object,
                new List<INotificationChannel>
                {
                    emailChannelMock.Object, pushChannelMock.Object, smsChannelMock.Object
                });

            // Act
            await service.DispatchAsync(message);

            // Assert
            emailChannelMock.Verify(
                x => x.SendAsync(user, message),
                Times.Once);

            smsChannelMock.Verify(
                x => x.SendAsync(user, message),
                Times.Once);

            pushChannelMock.Verify(
                x => x.SendAsync(user, message),
                Times.Once);

            _logRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<NotificationLog>()),
                Times.Exactly(3));
        }

        [Fact]
        public async Task DispatchAsync_Should_Log_Failure_When_Channel_Fails()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Channels = new List<Channel>
                {
                    new Channel
                    {
                        Id = (int)NotificationChannelType.Email,
                        ChannelType = NotificationChannelType.Email
                    }
                }
            };

            var message = new Message
            {
                CategoryId = 1
            };

            _userRepositoryMock
                .Setup(x => x.GetSubscribedUsersAsync(1))
                .ReturnsAsync(new List<User> { user });

            _channelMock
                .Setup(x => x.ChannelType)
                .Returns(NotificationChannelType.Email);

            _channelMock
                .Setup(x => x.SendAsync(user, message))
                .ThrowsAsync(new Exception("SMTP failed"));

            var service = new NotificationDispatcherService(
                _userRepositoryMock.Object,
                _logRepositoryMock.Object,
                new List<INotificationChannel>
                {
                    _channelMock.Object
                });

            // Act
            await service.DispatchAsync(message);

            // Assert
            _logRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.Is<NotificationLog>(l =>
                        l.Status == NotificationLogStatus.Failed)),
                Times.Once);
        }
    }
}
