namespace Client.Services;

public class DialogService
{
    public async Task DisplayAlert(string title, string message)
    {
        await Application.Current.Dispatcher.DispatchAsync(async () =>
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Close");
        });
    }
}