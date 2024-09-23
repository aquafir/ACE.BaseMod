namespace AutoLoot.Lib.VTClassic
{
    public class LootAction
    {
        internal string _ruleName = "";
        internal eLootAction _lootAction;
        internal int _data1;

        internal LootAction(eLootAction A_0)
        {
            _lootAction = A_0;
        }

        internal LootAction(eLootAction A_0, int A_1)
        {
            _lootAction = A_0;
            _data1 = A_1;
        }

        public int Data1
        {
            get
            {
                return _data1;
            }
        }

        public eLootAction SimpleAction
        {
            get
            {
                return _lootAction;
            }
        }

        public bool IsNoLoot
        {
            get
            {
                return _lootAction == eLootAction.NoLoot;
            }
        }

        public bool IsKeep
        {
            get
            {
                return _lootAction == eLootAction.Keep;
            }
        }

        public bool IsSalvage
        {
            get
            {
                return _lootAction == eLootAction.Salvage;
            }
        }

        internal bool IsRead
        {
            get
            {
                return _lootAction == eLootAction.Read;
            }
        }

        public bool IsKeepUpTo
        {
            get
            {
                return _lootAction == eLootAction.KeepUpTo;
            }
        }

        public bool IsSell
        {
            get
            {
                return _lootAction == eLootAction.Sell;
            }
        }

        public string RuleName
        {
            get
            {
                return _ruleName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _ruleName = value;
                else
                    _ruleName = "";
            }
        }

        public static LootAction NoLoot
        {
            get
            {
                return new LootAction(eLootAction.NoLoot);
            }
        }

        public static LootAction Keep
        {
            get
            {
                return new LootAction(eLootAction.Keep);
            }
        }

        public static LootAction Salvage
        {
            get
            {
                return new LootAction(eLootAction.Salvage);
            }
        }

        internal static LootAction Read
        {
            get
            {
                return new LootAction(eLootAction.Read);
            }
        }

        public static LootAction Sell
        {
            get
            {
                return new LootAction(eLootAction.Sell);
            }
        }

        public static LootAction User1
        {
            get
            {
                return new LootAction(eLootAction.User1);
            }
        }

        public static LootAction User2
        {
            get
            {
                return new LootAction(eLootAction.User2);
            }
        }

        public static LootAction User3
        {
            get
            {
                return new LootAction(eLootAction.User3);
            }
        }

        public static LootAction User4
        {
            get
            {
                return new LootAction(eLootAction.User4);
            }
        }

        public static LootAction User5
        {
            get
            {
                return new LootAction(eLootAction.User5);
            }
        }

        public static LootAction GetKeepUpTo(int maxcount)
        {
            return new LootAction(eLootAction.KeepUpTo, maxcount);
        }
    }
}