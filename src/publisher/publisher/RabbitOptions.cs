﻿namespace publisher
{
    public class RabbitOptions
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MessagesPerSecond { get; set; }
    }
}
