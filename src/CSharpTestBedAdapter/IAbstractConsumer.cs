/*************************************************************
 * Copyright (C) 2017-2018 
 *               XVR Simulation B.V., Delft, The Netherlands
 *               Martijn Hendriks <hendriks @ xvrsim.com>
 * 
 * This file is part of "DRIVER+ WP923 Test-bed infrastructure" project.
 * 
 * This file is licensed under the MIT license : 
 *   https://github.com/DRIVER-EU/test-bed/blob/master/LICENSE
 *
 *************************************************************/
using System;

namespace CSharpTestBedAdapter
{
    internal interface IAbstractConsumer
    {
        /// <summary>
        /// The type of messages this consumer can receive
        /// </summary>
        Type MessageType
        {
            get;
        }

        /// <summary>
        /// Method for re-receiving queued messages, stored because of this adapter being disabled
        /// </summary>
        void FlushQueue();
    }
}
