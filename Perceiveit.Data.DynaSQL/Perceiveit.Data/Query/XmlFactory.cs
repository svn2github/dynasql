/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Perceiveit.Data.Query
{
    public class XmlFactory
    {
        protected XmlFactory()
        {
        }

        public static XmlFactory Create()
        {
            return new XmlFactory();
        }

        public DBClause Read(string element, XmlReader reader, XmlReaderContext context)
        {
            return this.DoRead(element, reader, context);
        }

        protected virtual DBClause DoRead(string element, XmlReader reader, XmlReaderContext context)
        {
            DBClause c = null;

            switch (element)
            {
                case(XmlHelper.Select):
                    c = DBSelectQuery.Select();
                    break;

                case(XmlHelper.Delete):
                    c = DBDeleteQuery.Delete();
                    break;

                case(XmlHelper.Update):
                    c = DBUpdateQuery.Update();
                    break;

                case(XmlHelper.Insert):
                    c = DBInsertQuery.InsertInto();
                    break;

                case (XmlHelper.Script):
                    c = DBQuery.Script();
                    break;

                case(XmlHelper.Table):
                    c = DBTable.Table();
                    break;

                case(XmlHelper.Fields):
                    c = DBSelectSet.Select();
                    break;

                case(XmlHelper.AField):
                    c = DBField.Field();
                    break;

                case(XmlHelper.AllFields):
                    c = DBField.AllFields();
                    break;

                case(XmlHelper.From):
                    c = DBTableSet.From();
                    break;

                case(XmlHelper.Where):
                    c = DBFilterSet.Where();
                    break;

                case(XmlHelper.Group):
                    c = DBGroupBySet.GroupBy();
                    break;

                case(XmlHelper.Order):
                    c = DBOrderSet.OrderBy();
                    break;

                case(XmlHelper.Assignments):
                    c = DBAssignSet.Assign();
                    break;

                case(XmlHelper.Values):
                    c = DBValueSet.Values();
                    break;

                case(XmlHelper.Join):
                    c = DBJoin.Join();
                    break;

                case(XmlHelper.Function):
                    c = DBFunction.Function();
                    break;

                case(XmlHelper.Constant):
                    c = DBConst.Null();
                    break;

                case(XmlHelper.Top):
                    c = DBTop.Top();
                    break;

                case(XmlHelper.UnaryOp):
                    c = DBComparison.Not();
                    break;

                case(XmlHelper.Compare):
                    c = DBComparison.Compare();
                    break;

                case(XmlHelper.Between):
                    c = DBComparison.Between();
                    break;

                case(XmlHelper.Parameter):
                    //parameter is a special case.
                    //we add them to akeyed colection if they are not already registered
                    //then at the end we set the values at the end
                    string name = reader.GetAttribute(XmlHelper.Name);
                    DBParam aparam;
                    if (context.Parameters.TryGetParameter(name, out aparam))
                        c = aparam;
                    else
                    {
                        aparam = DBParam.Param();
                        aparam.Name = name;
                        context.Parameters.Add(aparam);
                        c = aparam;
                    }
                    break;

                case(XmlHelper.OrderBy):
                    c = DBOrder.OrderBy();
                    break;
                    
                case(XmlHelper.Calculation):
                    c = DBCalc.Calculate();
                    break;

                case(XmlHelper.Aggregate):
                    c = DBAggregate.Aggregate();
                    break;

                case(XmlHelper.ValueGroup):
                    c = DBValueGroup.Empty();
                    break;
                case(XmlHelper.BooleanOperator):
                    c = DBBooleanOp.Compare();
                    break;

                case(XmlHelper.Assign):
                    c = DBAssign.Assign();
                    break;

                case(XmlHelper.InnerSelect):
                    c = DBSubQuery.SubSelect();
                    break;

                case(XmlHelper.Multiple):
                    c = DBMultiComparisonRef.Many();
                    break;

                case(XmlHelper.QueryOptionSet):
                    c = new DBQueryHintOptionSet();
                    break;
                case(XmlHelper.QueryOption):
                    c = DBQueryHintOption.QueryOption();
                    break;

                case(XmlHelper.CreateSproc):
                    c = DBCreateProcedureQuery.CreateProcedure();
                    break;

                case(XmlHelper.Declare):
                    c = DBDeclaration.Declare();
                    break;

                case(XmlHelper.CreateTable):
                    c = DBCreateTableQuery.Table();
                    break;

                case(XmlHelper.ColumnDefinition):
                    c = DBColumn.Column();
                    break;

                case(XmlHelper.PrimaryKey):
                    c = DBPrimaryKey.PrimaryKey();
                    break;

                case(XmlHelper.ForeignKey):
                    c = DBForeignKey.ForeignKey();
                    break;

                case(XmlHelper.CreateIndex):
                    c = DBCreateIndexQuery.Index();
                    break;

                case(XmlHelper.CreateView):
                    c = DBCreateViewQuery.CreateView();
                    break;

                case(XmlHelper.CreateSequence):
                    c = DBCreateSequenceQuery.Sequence();
                    break;

                case(XmlHelper.DropTable):
                    c = DBDropTableQuery.DropTable();
                    break;

                case(XmlHelper.DropIndex):
                    c = DBDropIndexQuery.DropIndex();
                    break;

                case(XmlHelper.DropView):
                    c = DBDropViewQuery.DropView();
                    break;

                case(XmlHelper.DropSproc):
                    c = DBDropProcedureQuery.DropProcedure();
                    break;

                case(XmlHelper.DropSequence):
                    c = DBDropSequenceQuery.DropSequence();
                    break;
                default:
                    throw XmlHelper.CreateException(Errors.CantDeserializeXML, reader, null, element);

            }

            if(c != null)
                c.ReadXml(reader, context);

            return c;
        }
    }
}
