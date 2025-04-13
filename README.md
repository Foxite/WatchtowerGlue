# WatchtowerGlue
Use in conjunction with [Watchtower](https://github.com/containrrr/watchtower) to automate your deployments.

WatchtowerGlue is designed to be linked to a selfhosted [`registry`](https://github.com/distribution/distribution) container, which can be configured to call a webhook when images are pushed:
```yaml
# /etc/docker/registry/config.yml
notifications:
  events:
    includereferences: true
  endpoints:
    - name: updater
      url: http://glue-container-name/
      timeout: 1500ms
      threshold: 1
      backoff: 0s
      ignore:
        # Doesn't work, but whatever.
        mediatypes:
          - application/octet-stream
        actions:
          - pull


```
WatchtowerGlue is not supposed to be accessible from the internet, as it does not authenticate its incoming requests.

Environment variables:
- DEBOUNCE_MILLIS: the amount of milliseconds to wait before making a request to Watchtower. Useful if your workflows push multiple images, to avoid triggering multiple updates. Default 5000
- WATCHTOWER_TOKEN: the authorization token configured in Watchtower's http api.
- WATCHTOWER: the full hostname including http:// or https:// that Watchtower is located at.

# AuthenticatedGlue
AuthenticatedGlue is meant to be used in conjunction with the [trigger-update](https://github.com/Foxite/trigger-update) workflow action. It authenticates all incoming requests using JWTs.

## Environment variables
| Key               | Description                                                                                 | Example                            |
|-------------------|---------------------------------------------------------------------------------------------|------------------------------------|
| Keys__Keys__*     | followed by a key id, the value should be the PEM-encoded certificate for that signing key. | `-----BEGIN CERTIFICATE-----\n...` |
| Watchtower__Url   | The full URL that Watchtower is accessible from                                             | `http://localhost:8080`            |
| Watchtower__Token | The HTTP API token for Watchtower                                                           | `mytoken`                          |
