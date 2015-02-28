
namespace QiniuLab
{
    class Config
    {
        public const string API_HOST = "http://192.168.200.107/~jemy/qiniu-lab-php-v6";

        //simple upload
        public const string SIMPLE_UPLOAD_WITHOUT_KEY_UPTOKEN_PATH = "/demos/api/simple_upload_without_key_upload_token.php";
        public const string SIMPLE_UPLOAD_WITH_KEY_UPTOKEN_PATH = "/demos/api/simple_upload_with_key_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_SAVE_KEY_UPTOKEN_PATH = "/demos/api/simple_upload_use_save_key_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_SAVE_KEY_FROM_XPARAM_PATH = "/demos/api/simple_upload_use_save_key_from_xparam_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_RETURN_BODY_PATH = "/demos/api/simple_upload_use_return_body_upload_token.php";
        public const string SIMPLE_UPLOAD_OVERWRITE_EXISTING_FILE_PATH = "/demos/api/simple_upload_overwrite_existing_file_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_FSIZE_LIMIT_PATH = "/demos/api/simple_upload_use_fsize_limit_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_MIME_LIMIT_PATH = "/demos/api/simple_upload_use_mime_limit_upload_token.php";
        public const string SIMPLE_UPLOAD_WITH_MIMETYPE_PATH = "/demos/api/simple_upload_with_mimetype_upload_token.php";
        public const string SIMPLE_UPLOAD_WITH_CRC32_CHECK_PATH = "/demos/api/simple_upload_enable_crc32_check_upload_token.php";
        public const string SIMPLE_UPLOAD_USE_ENDUSER_PATH = "/demos/api/simple_upload_use_enduser_upload_token.php";

        // resumable upload 
        public const string RESUMABLE_UPLOAD_WITHOUT_KEY_PATH = "/demos/api/resumable_upload_without_key_upload_token.php";
        public const string RESUMABLE_UPLOAD_WITH_KEY_PATH = "/demos/api/resumable_upload_with_key_upload_token.php";

        // callback upload 
        public const string CALLBACK_UPLOAD_WITH_KEY_IN_URL_FORMAT_PATH = "/demos/api/callback_upload_with_key_in_url_format_upload_token.php";
        public const string CALLBACK_UPLOAD_WITH_KEY_IN_JSON_FORMAT_PATH = "/demos/api/callback_upload_with_key_in_json_format_upload_token.php";

    }
}
