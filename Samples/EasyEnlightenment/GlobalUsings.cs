﻿//extern alias Json;
global using ACE.Entity.Enum;
global using ACE.Server.Command;
global using ACE.Server.Entity;
global using ACE.Server.Mods;
global using ACE.Server.Network;
global using ACE.Server.Network.GameEvent.Events;
global using ACE.Server.WorldObjects;
global using ACE.Shared;
global using HarmonyLib;
global using System;
global using System.Reflection;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
#if REALM
global using Session = ACE.Server.Network.ISession;
global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
#endif
