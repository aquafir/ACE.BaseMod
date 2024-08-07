﻿//extern alias Json;
global using ACE.Entity.Enum;
global using ACE.Entity.Enum.Properties;
global using ACE.Server.Command;
global using ACE.Server.Entity;
global using ACE.Server.Managers;
global using ACE.Server.Mods;
global using ACE.Server.Network;
global using ACE.Server.WorldObjects;
global using ACE.Shared.Helpers;
global using Discord.Autocomplete;
global using Discord.Interactions;
global using HarmonyLib;
global using System;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using SummaryAttribute = Discord.Interactions.SummaryAttribute;
#if REALM
global using Session = ACE.Server.Network.ISession;
global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
#endif
