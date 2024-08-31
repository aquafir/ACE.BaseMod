//extern alias Json;
global using ACE.Common.Extensions;
global using ACE.Entity.Enum;

global using ACE.Server.Command;
global using ACE.Server.Mods;
global using ACE.Server.WorldObjects;
#if REALM
global using Session = ACE.Server.Network.ISession;
#endif

global using HarmonyLib;

global using System.Diagnostics;
global using System.Numerics;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using ACE.Shared.Helpers;

global using ImGuiNET;
