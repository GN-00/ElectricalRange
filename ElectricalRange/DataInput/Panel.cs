namespace ProjectsNow.DataInput
{
    public static class Panel
    {
        public static string ArabicType(string type)
        {
            if (type == "Power Panel")
            {
                return "لوحة توزيع كهربائية";
            }
            else if (type == "Control Panel")
            {
                return "لوحة تحكم كهربائية";
            }
            else if (type == "Ready Made Panel")
            {
                return "لوحة توزيع كهربائية";
            }
            else if (type == "ECB")
            {
                return "لوحة توزيع كهربائية";
            }
            else if (type == "ATS")
            {
                return "لوحة تحويل كهربائية أوتوماتيكية";
            }
            else if (type == "MTS")
            {
                return "لوحة تحويل كهربية يدوية";
            }
            else if (type == "MCC")
            {
                return "لوحة تحكم كهربائية";
            }
            else if (type == "PFC")
            {
                return "لوحة تصحيح معامل القدرة";
            }
            else if (type == "Synchronizing Panel")
            {
                return "لوحة مزامنة مولدات كهربائية";
            }
            else if (type == "Fuse Panel")
            {
                return "لوحة توزيع كهربائية";
            }
            else if (type == "Junction Box")
            {
                return "لوحة توزيع كهربائية";
            }
            else if (type == "Label")
            {
                return "مسيميات بلاستيكية";
            }
            else if (type == "Site Work")
            {
                return "عمل موقع";
            }
            else if (type == "Materials")
            {
                return "مواد";
            }
            else
            {
                return null;
            }
        }
    }
}
