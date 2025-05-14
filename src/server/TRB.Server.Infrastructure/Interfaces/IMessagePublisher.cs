﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Infrastructure.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}
