﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinOptionalVariable : GremlinTableVariable
    {
        public GremlinToSqlContext Context { get; set; }
        public GremlinVariable2 InputVariable { get; set; }

        public GremlinOptionalVariable(GremlinToSqlContext context, GremlinVariable2 inputVariable)
        {
            Context = context;
            VariableName = GenerateTableAlias();
            InputVariable = inputVariable;
        }

        public static GremlinOptionalVariable Create(GremlinVariable2 inputVariable, GremlinToSqlContext context)
        {
            if (inputVariable.GetVariableType() == context.PivotVariable.GetVariableType())
            {
                switch (context.PivotVariable.GetVariableType())
                {
                    case GremlinVariableType.Vertex:
                        return new GremlinOptionalVertexVariable(context, inputVariable);
                    case GremlinVariableType.Edge:
                        return new GremlinOptionalEdgeVariable(context, inputVariable);
                    case GremlinVariableType.Table:
                        return new GremlinOptionalTableVariable(context, inputVariable);
                    case GremlinVariableType.Scalar:
                        return new GremlinOptionalScalarVariable(context, inputVariable);
                }
            }
            else
            {
                return new GremlinOptionalTableVariable(context, inputVariable);
            }
            throw new NotImplementedException();
        }

        public override  WTableReference ToTableReference()
        {
            List<WScalarExpression> PropertyKeys = new List<WScalarExpression>();
            Dictionary<string, int> columns = new Dictionary<string, int>();
            if (InputVariable.DefaultProjection() is GremlinVariableProperty)
            {
                columns[(InputVariable.DefaultProjection() as GremlinVariableProperty).VariableProperty] = 0;

            }
            if (InputVariable is GremlinTableVariable)
            {
                var tableVar = InputVariable as GremlinTableVariable;
                foreach (var projectProperty in tableVar.ProjectedProperties)
                {
                    columns[projectProperty] = 0;
                }
            }
            if (Context.PivotVariable.DefaultProjection() is GremlinVariableProperty)
            {
                columns[(Context.PivotVariable.DefaultProjection() as GremlinVariableProperty).VariableProperty] = 1;
            }
            foreach (var projectProperty in ProjectedProperties)
            {
                columns[projectProperty] = 1;
            }
            
            WSelectQueryBlock firstQueryExpr = new WSelectQueryBlock();
            WSelectQueryBlock secondQueryExpr = Context.ToSelectQueryBlock();
            secondQueryExpr.SelectElements.Clear();
            foreach (var column in columns)
            {
                WScalarExpression scalarExpr;
                if (column.Value == 0)
                {
                    //The column comes from first query, so set the column of second query as null
                    scalarExpr = GremlinUtil.GetColumnReferenceExpr(InputVariable.VariableName, column.Key);
                    firstQueryExpr.SelectElements.Add(GremlinUtil.GetSelectScalarExpr(scalarExpr));

                    scalarExpr = GremlinUtil.GetNullExpr();
                    secondQueryExpr.SelectElements.Add(GremlinUtil.GetSelectScalarExpr(scalarExpr));
                }
                else
                {
                    //The column comes from second query, so set the column of first query as null
                    scalarExpr = GremlinUtil.GetNullExpr();
                    firstQueryExpr.SelectElements.Add(GremlinUtil.GetSelectScalarExpr(scalarExpr));

                    scalarExpr = GremlinUtil.GetColumnReferenceExpr(InputVariable.VariableName, column.Key);
                    secondQueryExpr.SelectElements.Add(GremlinUtil.GetSelectScalarExpr(scalarExpr));
                }
            }

            var WBinaryQueryExpression = GremlinUtil.GetBinaryQueryExpr(firstQueryExpr, secondQueryExpr);

            PropertyKeys.Add(GremlinUtil.GetScalarSubquery(WBinaryQueryExpression));
            var secondTableRef = GremlinUtil.GetFunctionTableReference("optional", PropertyKeys, VariableName);
            return GremlinUtil.GetCrossApplyTableReference(null, secondTableRef);
        }

        internal override void Populate(string property)
        {
            Context.Populate(property);
            base.Populate(property);
        }
    }

    internal class GremlinOptionalVertexVariable : GremlinOptionalVariable
    {
        public GremlinOptionalVertexVariable(GremlinToSqlContext context, GremlinVariable2 inputVariable): base(context, inputVariable) {}

        internal override GremlinVariableType GetVariableType()
        {
            return GremlinVariableType.Vertex;
        }
    }

    internal class GremlinOptionalEdgeVariable : GremlinOptionalVariable
    {
        public GremlinOptionalEdgeVariable(GremlinToSqlContext context, GremlinVariable2 inputVariable) : base(context, inputVariable) { }

        internal override GremlinVariableType GetVariableType()
        {
            return GremlinVariableType.Edge;
        }
    }

    internal class GremlinOptionalTableVariable : GremlinOptionalVariable
    {
        public GremlinOptionalTableVariable(GremlinToSqlContext context, GremlinVariable2 inputVariable) : base(context, inputVariable) { }

        internal override GremlinVariableType GetVariableType()
        {
            return GremlinVariableType.Table;
        }
    }

    internal class GremlinOptionalScalarVariable : GremlinOptionalVariable
    {
        public GremlinOptionalScalarVariable(GremlinToSqlContext context, GremlinVariable2 inputVariable) : base(context, inputVariable) { }

        internal override GremlinVariableType GetVariableType()
        {
            return GremlinVariableType.Scalar;
        }
    }
}