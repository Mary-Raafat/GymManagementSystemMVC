using System.Text.Json;

namespace GymManagementSystem.DAL.DataSeeding
{
    public static class SeedJsonLoader
    {
        public static List<T> LoadFromJson<T>(string fileName)
        {
            // الخطوة 1: نبني المسار الكامل للملف
            // AppContext.BaseDirectory => المجلد اللي التطبيق بيشتغل منه (bin/Debug/...)
            // Path.Combine        => بيجمع المسار مع اسم الملف
            var filePath = Path.Combine(AppContext.BaseDirectory, "DataSeeding", fileName);

            // الخطوة 2: بنتأكد إن الملف موجود فعلاً
            // لو مش موجود نرمي exception واضحة بدل ما البرنامج يقع بشكل غريب
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed file not found: {filePath}");

            // الخطوة 3: بنقرأ محتوى الملف كله كـ string نص عادي
            var jsonContent = File.ReadAllText(filePath);

            // الخطوة 4: بنحول الـ JSON string لـ List من Objects
            // JsonSerializer.Deserialize => بيحول الـ JSON لـ C# objects
            // <List<T>>               => بنقوله النوع اللي عايز ترجعه (مثلاً List<PlanSeedModel>)
            // PropertyNameCaseInsensitive => بيخلي الـ matching يشتغل بغض النظر عن الـ Capital/Small letters
            var result = JsonSerializer.Deserialize<List<T>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // الخطوة 5: لو الملف فاضي او الـ JSON مش صح نرجع قائمة فاضية بدل null
            return result ?? [];
        }
    }
}
