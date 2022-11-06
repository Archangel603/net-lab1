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
    
    public async Task<bool> DisplayConfirm(string title, string message)
    {
        var confirmed = false;
        
        await Application.Current.Dispatcher.DispatchAsync(async () =>
        {
            confirmed = await Application.Current.MainPage.DisplayAlert(title, message, "Confirm", "Cancel");
        });

        return confirmed;
    }
}