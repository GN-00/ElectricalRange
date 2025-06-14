using Dapper.Contrib.Extensions;

using System;
using System.IO;

namespace ProjectsNow.Data.JobOrders
{
    [Table("[JobOrder].[QualityPanels]")]
    public class QCPanel : Base
    {
        private DateTime _Date;
        private string _Voltage;
        private string _Current;
        private string _Enclosure;
        private string _Installation;
        private string _IP;
        private bool _VisualOk = true;
        private bool _VisualNotOk;
        private bool _VisualNA;
        private string _VisualNote;
        private bool _DiElectricOk = true;
        private bool _DiElectricNotOk;
        private bool _DiElectricNA;
        private string _DiElectricNote;
        private bool _InsulationOk = true;
        private bool _InsulationNotOk;
        private bool _InsulationNA;
        private string _InsulationNote;
        private bool _ContinuityOk = true;
        private bool _ContinuityNotOk;
        private bool _ContinuityNA;
        private string _ContinuityNote;
        private bool _OperationOk = true;
        private bool _OperationNotOk;
        private bool _OperationNA;
        private string _OperationNote;
        private string _Note;
        private string _Result = "Passed";

        [Key]
        public int Id { get; set; }
        public int PanelId { get; set; }

        public string Code { get; set; }
        public DateTime Date
        {
            get => _Date;
            set => SetValue(ref _Date, value);
        }

        public string Voltage
        {
            get => _Voltage;
            set => SetValue(ref _Voltage, value);
        }

        public string Current
        {
            get => _Current;
            set => SetValue(ref _Current, value);
        }

        public string Enclosure
        {
            get => _Enclosure;
            set => SetValue(ref _Enclosure, value);
        }

        public string Installation
        {
            get => _Installation;
            set => SetValue(ref _Installation, value);
        }

        public string IP
        {
            get => _IP;
            set => SetValue(ref _IP, value);
        }


        public bool VisualOk
        {
            get => _VisualOk;
            set
            {
                if (!value)
                {
                    if (!VisualNotOk && !VisualNA)
                        return;
                }

                if (SetValue(ref _VisualOk, value))
                {
                    if (VisualOk == true)
                    {
                        VisualNotOk = false;
                        VisualNA = false;
                    }
                }
            }
        }
        public bool VisualNotOk
        {
            get => _VisualNotOk;
            set
            {
                if (!value)
                {
                    if (!VisualOk && !VisualNA)
                        return;
                }

                if (SetValue(ref _VisualNotOk, value))
                {
                    if (VisualNotOk == true)
                    {
                        VisualOk = false;
                        VisualNA = false;
                    }
                }
            }
        }
        public bool VisualNA
        {
            get => _VisualNA;
            set
            {
                if (!value)
                {
                    if (!VisualOk && !VisualNotOk)
                        return;
                }

                if (SetValue(ref _VisualNA, value))
                {
                    if (VisualNA == true)
                    {
                        VisualOk = false;
                        VisualNotOk = false;
                    }
                }
            }
        }
        public string VisualNote
        {
            get => _VisualNote;
            set => SetValue(ref _VisualNote, value);
        }


        public bool DiElectricOk
        {
            get => _DiElectricOk;
            set
            {
                if (!value)
                {
                    if (!DiElectricNotOk && !DiElectricNA)
                        return;
                }

                if (SetValue(ref _DiElectricOk, value))
                {
                    if (DiElectricOk == true)
                    {
                        DiElectricNotOk = false;
                        DiElectricNA = false;
                    }
                }
            }
        }
        public bool DiElectricNotOk
        {
            get => _DiElectricNotOk;
            set
            {
                if (!value)
                {
                    if (!DiElectricOk && !DiElectricNA)
                        return;
                }

                if (SetValue(ref _DiElectricNotOk, value))
                {
                    if (DiElectricNotOk == true)
                    {
                        DiElectricOk = false;
                        DiElectricNA = false;
                    }
                }
            }
        }
        public bool DiElectricNA
        {
            get => _DiElectricNA;
            set
            {
                if (!value)
                {
                    if (!DiElectricOk && !DiElectricNotOk)
                        return;
                }

                if (SetValue(ref _DiElectricNA, value))
                {
                    if (DiElectricNA == true)
                    {
                        DiElectricOk = false;
                        DiElectricNotOk = false;
                    }
                }
            }
        }
        public string DiElectricNote
        {
            get => _DiElectricNote;
            set => SetValue(ref _DiElectricNote, value);
        }

        public bool InsulationOk
        {
            get => _InsulationOk;
            set
            {
                if (!value)
                {
                    if (!InsulationNotOk && !InsulationNA)
                        return;
                }

                if (SetValue(ref _InsulationOk, value))
                {
                    if (InsulationOk == true)
                    {
                        InsulationNotOk = false;
                        InsulationNA = false;
                    }
                }
            }
        }
        public bool InsulationNotOk
        {
            get => _InsulationNotOk;
            set
            {
                if (!value)
                {
                    if (!InsulationOk && !InsulationNA)
                        return;
                }

                if (SetValue(ref _InsulationNotOk, value))
                {
                    if (InsulationNotOk == true)
                    {
                        InsulationOk = false;
                        InsulationNA = false;
                    }
                }
            }
        }
        public bool InsulationNA
        {
            get => _InsulationNA;
            set
            {
                if (!value)
                {
                    if (!InsulationOk && !InsulationNotOk)
                        return;
                }

                if (SetValue(ref _InsulationNA, value))
                {
                    if (InsulationNA == true)
                    {
                        InsulationOk = false;
                        InsulationNotOk = false;
                    }
                }
            }
        }
        public string InsulationNote
        {
            get => _InsulationNote;
            set => SetValue(ref _InsulationNote, value);
        }

        public bool ContinuityOk
        {
            get => _ContinuityOk;
            set
            {
                if (!value)
                {
                    if (!ContinuityNotOk && !ContinuityNA)
                        return;
                }

                if (SetValue(ref _ContinuityOk, value))
                {
                    if (ContinuityOk == true)
                    {
                        ContinuityNotOk = false;
                        ContinuityNA = false;
                    }
                }
            }
        }
        public bool ContinuityNotOk
        {
            get => _ContinuityNotOk;
            set
            {
                if (!value)
                {
                    if (!ContinuityOk && !ContinuityNA)
                        return;
                }

                if (SetValue(ref _ContinuityNotOk, value))
                {
                    if (ContinuityNotOk == true)
                    {
                        ContinuityOk = false;
                        ContinuityNA = false;
                    }
                }
            }
        }
        public bool ContinuityNA
        {
            get => _ContinuityNA;
            set
            {
                if (!value)
                {
                    if (!ContinuityOk && !ContinuityNotOk)
                        return;
                }

                if (SetValue(ref _ContinuityNA, value))
                {
                    if (ContinuityNA == true)
                    {
                        ContinuityOk = false;
                        ContinuityNotOk = false;
                    }
                }
            }
        }
        public string ContinuityNote
        {
            get => _ContinuityNote;
            set => SetValue(ref _ContinuityNote, value);
        }


        public bool OperationOk
        {
            get => _OperationOk;
            set
            {
                if (!value)
                {
                    if (!OperationNotOk && !OperationNA)
                        return;
                }

                if (SetValue(ref _OperationOk, value))
                {
                    if (OperationOk == true)
                    {
                        OperationNotOk = false;
                        OperationNA = false;
                    }
                }
            }
        }
        public bool OperationNotOk
        {
            get => _OperationNotOk;
            set
            {
                if (!value)
                {
                    if (!OperationOk && !OperationNA)
                        return;
                }

                if (SetValue(ref _OperationNotOk, value))
                {
                    if (OperationNotOk == true)
                    {
                        OperationOk = false;
                        OperationNA = false;
                    }
                }
            }
        }
        public bool OperationNA
        {
            get => _OperationNA;
            set
            {
                if (!value)
                {
                    if (!OperationOk && !OperationNotOk)
                        return;
                }

                if (SetValue(ref _OperationNA, value))
                {
                    if (OperationNA == true)
                    {
                        OperationOk = false;
                        OperationNotOk = false;
                    }
                }
            }
        }
        public string OperationNote
        {
            get => _OperationNote;
            set => SetValue(ref _OperationNote, value);
        }


        public string Note
        {
            get => _Note;
            set => SetValue(ref _Note, value);
        }

        public string Result
        {
            get => _Result;
            set => SetValue(ref _Result, value);
        }


        [Write(false)]
        public string Customer { get; set; }

        [Write(false)]
        public string Project { get; set; }

        [Write(false)]
        public string JobOrderCode { get; set; }

        [Write(false)]
        public int SN { get; set; }

        [Write(false)]
        public string Name { get; set; }

        [Write(false)]
        public string NameInfo
        {
            get
            {
                using var reader = new StringReader(Name);
                return reader.ReadLine();
            }
        }


        [Write(false)]
        public string EnclosureType { get; set; }

        [Write(false)]
        public string EnclosureInstallation { get; set; }

        [Write(false)]
        public string EnclosureIP { get; set; }
    }
}
