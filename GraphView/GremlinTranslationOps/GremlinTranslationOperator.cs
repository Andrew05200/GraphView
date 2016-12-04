﻿using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using GraphView.GremlinTranslationOps;

namespace GraphView
{
    internal abstract class GremlinTranslationOperator
    {
        public List<string> Labels = new List<string>();
        public GremlinTranslationOperator InputOperator;
        public virtual GremlinToSqlContext GetContext()
        {
            return null;
        }
        public GremlinToSqlContext GetInputContext()
        {
            if (InputOperator != null) {
                return InputOperator.GetContext();
            } else {
                return new GremlinToSqlContext();
            }
        }
        public virtual WSqlScript ToSqlScript() {
            return GetContext().ToSqlScript();
        }

        public List<string> GetLabels()
        {
            return Labels;
        }

        public void ClearLabels()
        {
            Labels.Clear();
        }
    }
    
    internal class GremlinParentContextOp : GremlinTranslationOperator
    {
        public GremlinVariable InheritedVariable { get; set; }
        public List<Projection> InheritedProjection;
        public bool IsInheritedEntireContext = false;
        public GremlinToSqlContext InheritedContext;

        public void SetContext(GremlinToSqlContext context)
        {
            IsInheritedEntireContext = true;
            InheritedContext = context;
            InheritedProjection = new List<Projection>();
        }
        public override GremlinToSqlContext GetContext()
        {
            if (IsInheritedEntireContext) return InheritedContext;
            GremlinToSqlContext newContext = new GremlinToSqlContext();
            newContext.RootVariable = InheritedVariable;
            newContext.SetCurrVariable(InheritedVariable);
            newContext.FromOuter = true;
            newContext.ProjectionList = InheritedProjection;
            return newContext;
        }
    }
}