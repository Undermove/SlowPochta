﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Data.Model
{
    public class MessagePassedDeliveryStatus
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int DeliveryStatusVariantId { get; set; }

    }
}