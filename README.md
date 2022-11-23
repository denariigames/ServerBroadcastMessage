# Server Broadcast Message

![ServerBroadcastMessage](https://user-images.githubusercontent.com/755461/203473542-0b60e59b-fdfa-4ec1-bff6-1d47a04bc899.png)

Addon for [MMORPG Kit](https://assetstore.unity.com/packages/templates/systems/mmorpg-kit-2d-3d-survival-110188) to send server-generated message to specific player, players near specific player or guild of specific player.

There is no UI or config, all usage is done via code. Example usage from BasePlayerCharacterEntity:

```
#if UNITY_EDITOR || UNITY_SERVER
BaseGameNetworkManager.Singleton.ServerBroadcastMessage(this.Title, $"You have been killed by {attackerCharacter.Title}.", BroadcastTo.PlayerOnly);
#endif
```