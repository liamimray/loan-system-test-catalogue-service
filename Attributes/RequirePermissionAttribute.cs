namespace CatalogueService.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute
    {
        public string[] Permissions { get; }
        public bool RequireAll { get; }

        public RequirePermissionAttribute(params string[] permissions)
        {
            Permissions = permissions;
            RequireAll = false;
        }

        public RequirePermissionAttribute(bool requireAll, params string[] permissions)
        {
            Permissions = permissions;
            RequireAll = requireAll;
        }
    }
}
