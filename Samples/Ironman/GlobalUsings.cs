﻿//extern alias Json;
global using ACE.Common;
global using ACE.Database;
global using ACE.Database.Models.Shard;
global using ACE.DatLoader;
global using ACE.Entity.Enum;
global using ACE.Server.Command;
global using ACE.Server.Entity;

global using ACE.Server.Managers;
global using ACE.Server.Mods;
global using ACE.Server.Network;
global using ACE.Server.Network.GameEvent.Events;
global using ACE.Server.WorldObjects;
global using ACE.Shared;
global using ACE.Shared.Helpers;
global using HarmonyLib;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System;
global using System.Reflection;
global using System.Text;
global using System.Text.Encodings.Web;

#if REALM
global using ACE.Server.Realms;
global using Session = ACE.Server.Network.ISession;
global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
#endif