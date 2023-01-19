using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESAPIScript
{
    public enum WarningLevels
    {
        [Description("Generic script error")] GenericFailure,
    }

    public enum DICOMTypes
    {
        CONTROL,
        AVOIDANCE,
        CAVITY,
        CONTRAST_AGENT,
        CTV,
        EXTERNAL,
        GTV,
        IRRAD_VOLUME,
        ORGAN,
        PTV,
        TREATED_VOLUME,
        SUPPORT,
        FIXATION,
        DOSE_REGION
    }
}
