namespace GuardianStock.API.Infrastructure
{
    public enum ApplicationStatusCodes
    {
        // --- 2xx Success (العمليات التي تمت بنجاح) ---
        Ok = 200,                // النجاح القياسي (GET, PUT, PATCH)
        Created = 201,           // تم إنشاء مورد جديد (POST)
        Accepted = 202,          // تم استلام الطلب وبدأت المعالجة (مفيد في الـ Background Jobs)
        NoContent = 204,         // العملية تمت بنجاح ولكن لا يوجد محتوى للرد (DELETE)

        // --- 4xx Client Errors (أخطاء من طرف المستخدم) ---
        BadRequest = 400,        // المدخلات غير سليمة أو الـ Validation فشل
        Unauthorized = 401,      // المستخدم غير مسجل دخول (Missing/Invalid Token)
        Forbidden = 403,         // المستخدم مسجل دخول ولكن ليس لديه صلاحية لهذا الفعل
        NotFound = 404,          // المورد المطلوب (ID) غير موجود
        MethodNotAllowed = 405,  // الـ HTTP Method غير مدعومة لهذا الـ Endpoint
        RequestTimeout = 408,    // انتهت مهلة انتظار الطلب
        Conflict = 409,          // تضارب في البيانات (مثل تكرار الإيميل أو الـ SKU)
        UnsupportedMediaType = 415, // نوع الملف غير مدعوم (رفع PDF بدل CSV)
        UnprocessableEntity = 422,  // البيانات صحيحة كفورمات لكن مرفوضة بيزنس (Business Rules)
        TooManyRequests = 429,   // تجاوز عدد الطلبات المسموح بها (Rate Limiting)

        // --- 5xx Server Errors (أخطاء من طرف السيرفر) ---
        InternalServerError = 500, // خطأ غير متوقع في الكود أو قاعدة البيانات
        BadGateway = 502,          // مشكلة في السيرفر الوسيط أو الـ Load Balancer
        ServiceUnavailable = 503,  // السيرفر غير قادر على المعالجة حالياً (صيانة/ضغط)
        GatewayTimeout = 504       // السيرفر الوسيط لم يتلقَ رداً سريعاً من السيرفر الأصلي
    }
}
