﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinLocalOp: GremlinTranslationOperator
    {
        public GraphTraversal2 LocalTraversal { get; set; }

        public GremlinLocalOp(GraphTraversal2 localTraversal)
        {
            LocalTraversal = localTraversal;
        }

        public override GremlinToSqlContext GetContext()
        {
            GremlinToSqlContext inputContext = GetInputContext();

            GremlinUtil.InheritedVariableFromParent(LocalTraversal, inputContext);
            GremlinToSqlContext localContext = LocalTraversal.GetEndOp().GetContext();

            inputContext.PivotVariable.Local(inputContext, localContext);

            return inputContext;
        }
    }
}