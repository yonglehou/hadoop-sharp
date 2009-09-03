using System;

namespace Hadoop.Pipes
{
    public enum BinaryMessageTypes : int
    {
        START_MESSAGE = 0,
        SET_JOB_CONF,
        SET_INPUT_TYPES,
        RUN_MAP,
        MAP_ITEM,
        RUN_REDUCE,
        REDUCE_KEY,
        REDUCE_VALUE,
        CLOSE,
        ABORT,
        OUTPUT = 50,
        PARTITIONED_OUTPUT = 51,
        STATUS = 52,
        PROGRESS = 53,
        DONE = 54,
        REGISTER_COUNTER = 55,
        INCREMENT_COUNTER = 56,
    }
}
