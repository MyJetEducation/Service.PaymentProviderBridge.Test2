﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDepositRepository.Domain.Models;
using Service.PaymentProviderBridge.Test2.Models;
using Service.PaymentProviderBridge.Test2.Settings;

namespace Service.PaymentProviderBridge.Test2.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		private readonly ILogger<PaymentProviderGrpcService> _logger;
		private static readonly HttpClient Client;

		static PaymentProviderGrpcService() => Client = new HttpClient();

		public PaymentProviderGrpcService(ILogger<PaymentProviderGrpcService> logger) => _logger = logger;

		public ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			SettingsModel settings = Program.Settings;

			string externalUrl = SetTransactionId(settings.ServiceUrl, request.TransactionId);

			DepositRegisterResponse registerResponse;
			using (HttpResponseMessage response = Client.GetAsync(externalUrl).Result)
				using (HttpContent content = response.Content)
					registerResponse = JsonConvert.DeserializeObject<DepositRegisterResponse>(content.ReadAsStringAsync().Result);

			if (registerResponse == null)
			{
				_logger.LogError("External id not recieved for url {url} with request {@request}!", externalUrl, request);

				return ValueTask.FromResult(new ProviderDepositGrpcResponse
				{
					State = TransactionState.Error
				});
			}

			_logger.LogDebug("Accepted response {@registerResponse} by url {url} for request {@request}!", registerResponse, externalUrl, request);

			TransactionState state = GetState(registerResponse.State);

			return ValueTask.FromResult(new ProviderDepositGrpcResponse
			{
				State = state,
				ExternalId = registerResponse.ExternalId,
				RedirectUrl = state == TransactionState.Approved ? settings.OkUrl : settings.FailUrl
			});
		}

		private static TransactionState GetState(string state) =>
			state switch {
				"accept" => TransactionState.Accepted,
				"reject" => TransactionState.Rejected,
				"approve" => TransactionState.Approved, 
				_ => TransactionState.Error};

		private static string SetTransactionId(string urlTemplate, Guid? id) => urlTemplate.Replace("#transaction-id#", id.ToString(), StringComparison.OrdinalIgnoreCase);
	}
}