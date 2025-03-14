﻿using System;
using MoreMountains.Tools;

namespace Domains.Player.Scripts
{
    [Serializable]
    public enum PlayerStatusEventType
    {
        OutOfStamina,
        RegainedStamina,
        OutOfHealth,
        RegainedHealth,
        ImmuneToDamage
    }

    public struct PlayerStatusEvent
    {
        private static PlayerStatusEvent e;

        public PlayerStatusEventType EventType;

        public static void Trigger(PlayerStatusEventType eventType)
        {
            e.EventType = eventType;
            MMEventManager.TriggerEvent(e);
        }
    }
}