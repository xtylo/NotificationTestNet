using Application.Abstractions;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class NotificationDispatcherService : INotificationDispatcherService
    {

        private const int MAX_ATTEMPTS = 3;

        private readonly IUserRepository _userRepository;
        private readonly INotificationLogRepository _logRepository;
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationDispatcherService(
            IUserRepository userRepository,
            INotificationLogRepository logRepository,
            IEnumerable<INotificationChannel> channels)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _channels = channels;
        }

        public async Task DispatchAsync(Message message)
        {
            var users = await _userRepository
                .GetSubscribedUsersAsync(message.CategoryId);

            foreach (var user in users)
            {
                foreach (var userChannel in user.Channels)
                {
                    var channel = _channels.FirstOrDefault(x =>
                        x.ChannelType == userChannel.ChannelType);

                    if (channel == null)
                    {
                        await _logRepository.CreateAsync(
                            NotificationLog.ChannelNotConfigured(
                                user,
                                message,
                                userChannel,
                                $"Channel {userChannel.ChannelType} implementation not found"));
                        continue;
                    }

                    try
                    {
                        for (var attempt = 1; attempt <= 3; attempt++)
                        {
                            try
                            {
                                await channel.SendAsync(user, message);

                                await _logRepository.CreateAsync(
                                    NotificationLog.Success(
                                        user,
                                        message,
                                        userChannel,
                                        attempt - 1));

                                break;
                            }
                            catch(Exception ex)
                            {
                                if (attempt == MAX_ATTEMPTS)
                                    throw;

                                await Task.Delay(
                                    TimeSpan.FromSeconds(attempt));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logRepository.CreateAsync(
                            NotificationLog.Failure(
                                user,
                                message,
                                userChannel,
                                ex.Message, 
                                MAX_ATTEMPTS));
                    }
                }
            }
        }
    }
}
