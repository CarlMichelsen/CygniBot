# CygniBot
A Slack bot for Cygni Copenhagen. (Maybe we'll let the swedes use it too)

## Development environment setup

### Requirements
- A C# .net10 installation (https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- A tunnelling tool is required for development in order to do interactivity - try ```ngrok``` (https://ngrok.com/download)
- Whatever IDE you prefer
- Snacks :)

### Secrets during development
Add a json file called ```secrets.json``` to the ```App``` project.
There is a secrets template file called ```secrets.template.json```.

Whenever another secret is required to run CygniBot locally - please add it to the ```secrets.template.json``` file :)

## Installing the bot in Slack

### Create the Slack App
1. Go to https://api.slack.com/apps
2. Click Create New App → From scratch
3. Give it a name (e.g. ```CygniBot``` or ```DevCygniBot```)
4. Choose your workspace (or create a new development workspace first for testing)

### Configure OAuth & Permissions
1. In the left sidebar, select OAuth & Permissions
2. Scroll down to Bot Token Scopes and click Add an OAuth Scope
3. Add the following scopes: ```chat:write```, ```chat:write.public```, ```commands```, ```users:read``` (does not use commands yet)
4. Scroll back to the top and click Install to Workspace
5. Slack will prompt for authorization — approve it.
6. After installation, copy the Bot User OAuth Token (starts with xoxb-) use it in the CygniBot configuration ```Slack__BotToken```.


### Enable Interactivity
1. In the sidebar, click Interactivity & Shortcuts
2. Toggle Interactivity → ON
3. In Request URL, enter the public endpoint that Slack can reach. (**Important**: add ```api/v1/slack``` at the end of the domain)
4. If you’re developing locally, use ngrok (https://ngrok.com/download)
```bash
ngrok http http://localhost:5089
```
Copy the HTTPS URL that ngrok shows and append /slack/interact, for example
```bash
example: https://abcd1234.ngrok.io/slack/interact
```
5. Click Save Changes (Slack will verify the endpoint right away — if it’s valid, you’ll see a ✅ checkmark next to it.)

### Add a slash command (there are no commands yet - this step should be skipped)
1. Go to Slash Commands → Create New Command
2. Choose a command name (e.g. ```/weekly-report```)
3. Set the Request URL to the same base address as above (e.g. ```/slack/command```)
4. Click Save

### Reinstall the App (after config changes)
Whenever you modify scopes or features, Slack requires you to reinstall the app:
- Go to OAuth & Permissions
- Click Reinstall to Workspace

### Verify Installation
You should now be able to:
- Mention your bot in a channel or DM it
- See it listed under Apps in Slack
- Run any slash commands (if configured)
- Click interactive buttons in messages (Slack should POST to your /slack/interact endpoint)