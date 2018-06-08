﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView.Transaction
{
    class WriteSetEntry : TxSetEntry
    {
        internal object Payload { get; set; }

        internal long VersionKey { get; set; }

        public WriteSetEntry()
        {

        }

        public WriteSetEntry(object payload)
        {
            this.Payload = payload;
        }

        public WriteSetEntry(string tableId, object recordKey, object payload)
            : base(tableId, recordKey)
        {
            this.Payload = payload;
            this.VersionKey = 0;
        }

        public WriteSetEntry(string tableId, object recordKey, object payload, long versionKey)
            : base(tableId, recordKey)
        {
            this.Payload = payload;
            this.VersionKey = versionKey;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}