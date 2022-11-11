public interface IService { }

public static class ServiceHub<T> where T : IService {
    static T s_instance;
    public static T Instance => s_instance;

    public static void Bind(T service) {
        s_instance = service;
    }
}
