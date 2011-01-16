using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.SchemaTests
{
    public interface ISchemaEditor
    {

        System.Windows.Forms.Control UIControl { get; }

        Perceiveit.Data.Schema.DBSchemaItem Item { get; set; }
    }
}
