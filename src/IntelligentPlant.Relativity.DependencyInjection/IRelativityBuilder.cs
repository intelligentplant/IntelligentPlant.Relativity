using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.Relativity.DependencyInjection {

    /// <summary>
    /// Builder for configuring Relativity services.
    /// </summary>
    public interface IRelativityBuilder {

        /// <summary>
        /// The service collection.
        /// </summary>
        IServiceCollection Services { get; }

    }

}
