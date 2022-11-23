/**
 * BaseGameNetworkManager_ServerBroadcastMessage
 * Author: Denarii Games
 * Version: 1.0
 *
 * Send server-generated messages to specific player or players near specific player.
 * Example usage from BasePlayerCharacterEntity:
 *
 * #if UNITY_EDITOR || UNITY_SERVER
 * BaseGameNetworkManager.Singleton.ServerBroadcastMessage(this.Title, $"You have been killed by {attackerCharacter.Title}.", BroadcastTo.PlayerOnly);
 * #endif
 */

using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

namespace MultiplayerARPG
{
	public enum BroadcastTo {
		PlayerOnly,
		LocalPlayers,
		PlayerAndLocalPlayers,
		PlayerGuild
	}

	public abstract partial class BaseGameNetworkManager
	{
		public void ServerBroadcastMessage(string playerName, string message, BroadcastTo broadcastTo)
		{
			IPlayerCharacterData playerCharacter = null;
			GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(playerName, out playerCharacter);
			BasePlayerCharacterEntity playerCharacterEntity = playerCharacter == null ? null : playerCharacter as BasePlayerCharacterEntity;

			if (playerCharacterEntity != null)
			{
				if (broadcastTo == BroadcastTo.PlayerOnly || broadcastTo == BroadcastTo.PlayerAndLocalPlayers)
				{
					ServerSendPacket(playerCharacterEntity.ConnectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.Chat, new ChatMessage()
					{
						channel = ChatChannel.System,
						message = message,
					});
				}

				if (broadcastTo == BroadcastTo.LocalPlayers || broadcastTo == BroadcastTo.PlayerAndLocalPlayers)
				{
					List<BasePlayerCharacterEntity> receivers = playerCharacterEntity.FindCharacters<BasePlayerCharacterEntity>(GameInstance.Singleton.localChatDistance, false, true, true, true);
					foreach (BasePlayerCharacterEntity receiver in receivers)
					{
						ServerSendPacket(receiver.ConnectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.Chat, new ChatMessage()
						{
							channel = ChatChannel.System,
							message = message,
						});
					}
				}

				if (broadcastTo == BroadcastTo.PlayerGuild)
				{
					if (playerCharacter.GuildId > 0)
					{
						GuildData guild;
						long connectionId;
						if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guild))
						{
							foreach (string memberId in guild.GetMemberIds())
							{
								if (GameInstance.ServerUserHandlers.TryGetConnectionId(memberId, out connectionId))
								{
									ServerSendPacket(connectionId, 0, DeliveryMethod.ReliableOrdered, GameNetworkingConsts.Chat, new ChatMessage()
									{
										channel = ChatChannel.System,
										message = message,
									});
								}
							}
						}
					}
				}
			}
		}
	}
}