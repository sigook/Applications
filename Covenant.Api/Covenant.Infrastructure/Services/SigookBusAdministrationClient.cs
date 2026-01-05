using Azure.Messaging.ServiceBus.Administration;

namespace Covenant.Infrastructure.Services
{
    public class SigookBusAdministrationClient : ServiceBusAdministrationClient
    {
        public SigookBusAdministrationClient(string connectionString)
            : base(connectionString)
        {
        }

        public async Task CreateQueueIfNotExistsAsync(string queueName)
        {
            if (!await QueueExistsAsync(queueName))
            {
                await CreateQueueAsync(queueName);
            }
        }

        public async Task CreateTopicIfNotExistsAsync(string topicName)
        {
            if (!await TopicExistsAsync(topicName))
            {
                await CreateTopicAsync(topicName);
            }
        }

        public async Task CreateSubscriptionIfNotExistsAsync(string topic, string subscription)
        {
            if (!await SubscriptionExistsAsync(topic, subscription))
            {
                await CreateSubscriptionAsync(topic, subscription);
            }
        }

        public async Task CreateRuleIfNotExistsAsync(string topic, string subscription, string ruleName, CorrelationRuleFilter rule)
        {
            await DeleteDefaultRuleIfExistsAsync(topic, subscription);
            if (!await RuleExistsAsync(topic, subscription, ruleName))
            {
                await CreateRuleAsync(topic, subscription, new CreateRuleOptions(ruleName, rule));
            }
        }

        public async Task CreateRuleIfNotExistsAsync(string topic, string subscription, string ruleName, SqlRuleFilter rule)
        {
            await DeleteDefaultRuleIfExistsAsync(topic, subscription);
            if (!await RuleExistsAsync(topic, subscription, ruleName))
            {
                await CreateRuleAsync(topic, subscription, new CreateRuleOptions(ruleName, rule));
            }
        }

        private async Task DeleteDefaultRuleIfExistsAsync(string topic, string subscription)
        {
            if (await RuleExistsAsync(topic, subscription, "$Default"))
            {
                await DeleteRuleAsync(topic, subscription, "$Default");
            }
        }
    }
}
