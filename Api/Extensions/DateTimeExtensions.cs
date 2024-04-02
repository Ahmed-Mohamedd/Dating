namespace Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var TodaysDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var age  = TodaysDate.Year - dob.Year;
            if (dob > TodaysDate.AddYears(-age)) age--;
            return age;
        }
    }
}
