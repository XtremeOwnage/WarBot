import { BotCommonConfig } from './BotCommonConfig';
import { Bot_Updated } from './Discord_FormattedMessages';
import * as LOG from './Discord_Logging';


let CurrentVersion: number = 2.0;
let ReleaseNotesURL: string = "https://xtremeownage.com/index.php?threads/version-2-0.20591/";
let SendUpdateNotification: boolean = true;

export async function UpdateCheck(cfg: BotCommonConfig) {

    if (cfg.BotVersion && cfg.BotVersion != CurrentVersion) {
        let SentUpdate: boolean = false;

        //Alert the officers channel.... if defined.
        if (cfg.CH_Officers && SendUpdateNotification == true && (!cfg.Notifications || cfg.Notifications.SendUpdateMessage)) {
            SentUpdate = true;

            await cfg.CH_Officers.send(Bot_Updated(CurrentVersion, ReleaseNotesURL));
        }

        //Log to the admin server.
        await LOG.Guild_Updated_async(cfg.Guild, cfg.BotVersion, CurrentVersion, SentUpdate);

        //Set the bot's current version in the config.
        cfg.BotVersion = CurrentVersion;
    } else if (!cfg.BotVersion) {
        //There was no version stored? Set the version.
        cfg.BotVersion = CurrentVersion;
    }
}