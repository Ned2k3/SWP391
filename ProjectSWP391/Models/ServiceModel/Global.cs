using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProjectSWP391.Models.ServiceModel
{
    public static class Global
    {
        public static Account? CurrentUser { get; set; } = null;

    }
}
