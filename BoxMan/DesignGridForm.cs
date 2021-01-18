using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxMan
{
    /*
    class DesignGridForm : XNAForm
    {
        int _numRows, _numCols, _tileSize;

        public static DesignGridForm MakeDesignGridForm(int numRows, int numCols, int tileSize, FormMgr parent)
        {
            var form = new DesignGridForm(numRows, numCols, tileSize, parent);
            if (!form._initialize())
            {
                throw new InvalidCastException("State manager must be of type LevelDesigner");
                return null;
            }
            else
                return form;
        }

        DesignGridForm(int numRows, int numCols, int tileSize, FormMgr parent) : base(100, 100, numCols * tileSize, numRows*tileSize, parent, "", false)
        {
            _numRows = numRows;
            _numCols = numCols;
            _tileSize = tileSize;

        }

        private bool _initialize()
        {
            var stateMgr = _stateMgr as LevelDesigner;
            if (stateMgr == null)
                return false;
            for (int row = 0; row < _numRows; row++)
            {
                for (int col = 0; col < _numCols; col++)
                {
                    var painter = new PaintButton(col * _tileSize, row * _tileSize, _tileSize, _tileSize, this);
                    painter.EventCalls += stateMgr.CellButtonClicked;
                }
            }
            return true;
        }
    }
    */
}
