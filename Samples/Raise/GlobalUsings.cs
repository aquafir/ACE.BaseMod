global using ACE.Common;
global using ACE.Common.Extensions;
global using ACE.Database.Models.Shard;
global using ACE.DatLoader;
global using ACE.DatLoader.Entity.AnimationHooks;
global using ACE.Entity;
global using ACE.Entity.Enum;
global using ACE.Entity.Enum.Properties;
global using ACE.Entity.Models;

global using ACE.Server.Command;
global using ACE.Server.Entity;
global using ACE.Server.Entity.Actions;
global using ACE.Server.Factories;
global using ACE.Server.Factories.Enum;
global using ACE.Server.Managers;
global using ACE.Server.Mods;
global using ACE.Server.Network.GameEvent.Events;
global using ACE.Server.Network.GameMessages.Messages;
global using ACE.Server.Network;
global using ACE.Server.Physics;
global using ACE.Server.WorldObjects.Entity;
global using ACE.Server.WorldObjects;
#if REALM
global using ACE.Server.Realms;
//global using ACE.Database.Models.World;
global using Session = ACE.Server.Network.ISession;
global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
global using Position = ACE.Server.Realms.InstancedPosition;
#endif

global using HarmonyLib;

global using System.Diagnostics;
global using System.Numerics;
global using System.Reflection;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using ACE.Shared;
global using ACE.Shared.Helpers;
global using ACE.Shared.Mods;

