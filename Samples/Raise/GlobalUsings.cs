//extern alias Json;
global using ACE.DatLoader;
global using ACE.Entity.Enum.Properties;
global using ACE.Entity.Enum;
global using ACE.Server.Command;
global using ACE.Server.Managers;
global using ACE.Server.Mods;
global using ACE.Server.Network;
global using ACE.Server.Network.GameMessages.Messages;
global using ACE.Server.WorldObjects;
global using ACE.Server.WorldObjects.Entity;

global using HarmonyLib;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
#if REALM
global using Session = ACE.Server.Network.ISession;
#endif
