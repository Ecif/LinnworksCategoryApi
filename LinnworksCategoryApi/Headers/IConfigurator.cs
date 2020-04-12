namespace LinnworksCategoryApi.Headers
{
    public interface IConfigurator<in TFrom, in TTo>
    {
        void Apply(TFrom source, TTo target);
    }
}