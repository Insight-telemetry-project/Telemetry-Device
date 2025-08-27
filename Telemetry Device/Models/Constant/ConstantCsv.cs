using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Constant
{
    public class ConstantCsv
    {

        public const int HEADER_ROW_INDEX = 0;
        public const char CSV_DELIMITER = ',';
        public const int MIN_ROWS_REQUIRED = 2;
        public const int EMPTY_ROW_COUNT = 0;
        public const int FIRST_COLUMN_INDEX = 0;
        public const int DATA_START_ROW_INDEX = 1;
        public const string CLUSTER_PREFIX = "c_";
        public const int CLUSTER_PREFIX_LENGTH = 2;
    }
}
