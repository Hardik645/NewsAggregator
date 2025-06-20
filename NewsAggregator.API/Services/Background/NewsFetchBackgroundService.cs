using NewsAggregator.API.Services.Clients;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services.Background
{
    public class NewsFetchBackgroundService(
        IServiceProvider serviceProvider,
        NewsApiClientFactory clientFactory,
        ILogger<NewsFetchBackgroundService> logger) : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromHours(3);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("News fetch background service started.");
            using (var scope = serviceProvider.CreateScope())
            {
                var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                var sourceRepository = scope.ServiceProvider.GetRequiredService<ISourceRepository>();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var sources = await sourceRepository.GetAllSourcesAsync();
                        foreach (var source in sources)
                        {
                            try
                            {
                                var client = clientFactory.GetClient(source);
                                var articles = await client.FetchArticlesAsync();
                                await articleRepository.SaveArticlesAsync(articles);
                                await sourceRepository.UpdateActiveStatusAsync(source, true);
                                await articleRepository.CategorizeUnknownArticlesByKeywordsAsync();
                                await notificationRepository.AddCategoryAndKeywordNotificationsForArticlesAsync(articles);
                                logger.LogInformation("Fetched and stored articles for source: {Name}", source.Name);
                            }
                            catch (Exception ex)
                            {
                                await sourceRepository.UpdateActiveStatusAsync(source, false);
                                logger.LogError(ex, "Error fetching articles for source: {Name}", source.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error occurred in background news fetch job.");
                    }

                    await Task.Delay(_interval, stoppingToken);
                }
            }
            logger.LogInformation("News fetch background service stopped.");
        }
    }
}