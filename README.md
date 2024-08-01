# github-action demo

## memorandum

run shell file in workflow should make sure add execute permission.

```bash
git update-index --chmod=+x <your_script>.sh
```

## use act to run actions locally

## use podman over wsl

Get the target podman machine. I will use podman-machine-2.

```powershell
$ podman system connection list
Name                         URI                                                          Identity                                        Default
podman-machine-2             ssh://user@127.0.0.1:36798/run/user/1000/podman/podman.sock  C:\Users\zhangchao\.ssh\podman-machine-2        false
podman-machine-2-root        ssh://root@127.0.0.1:36798/run/podman/podman.sock            C:\Users\zhangchao\.ssh\podman-machine-2        false
podman-machine-default       ssh://user@127.0.0.1:30986/run/user/1000/podman/podman.sock  C:\Users\zhangchao\.ssh\podman-machine-default  true
podman-machine-default-root  ssh://root@127.0.0.1:30986/run/podman/podman.sock            C:\Users\zhangchao\.ssh\podman-machine-default  false
```

run act and specify the container socket and image

```powershell
$ act --container-daemon-socket ssh://user@127.0.0.1:36798/run/user/1000/podman/podman.sock -P ubuntu-latest=catthehacker/ubuntu:rust-latest
[Rust/build-1] 🚀  Start image=catthehacker/ubuntu:rust-latest
[Rust/build-3] 🚧  Skipping unsupported platform -- Try running with `-P macos-latest=...`
[Rust/build-2] 🚧  Skipping unsupported platform -- Try running with `-P windows-latest=...`
time="2024-02-21T15:38:33+08:00" level=info msg="Parallel tasks (0) below minimum, setting to 1"
[Rust/build-1]   🐳  docker pull image=catthehacker/ubuntu:rust-latest platform= username= forcePull=true
time="2024-02-21T15:38:37+08:00" level=info msg="Parallel tasks (0) below minimum, setting to 1"
[Rust/build-1]   🐳  docker create image=catthehacker/ubuntu:rust-latest platform= entrypoint=["tail" "-f" "/dev/null"] cmd=[] network="host"
[Rust/build-1]   🐳  docker run image=catthehacker/ubuntu:rust-latest platform= entrypoint=["tail" "-f" "/dev/null"] cmd=[] network="host"
[Rust/build-1] 🧪  Matrix: map[os:ubuntu-latest]
[Rust/build-1] ⭐ Run Main actions/checkout@v3
[Rust/build-1]   🐳  docker cp src=C:\Users\zhangchao\Source\github\zhangchao\github-action-demo\. dst=/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo
[Rust/build-1]   ✅  Success - Main actions/checkout@v3
[Rust/build-1] ⭐ Run Main Build
[Rust/build-1]   🐳  docker exec cmd=[bash --noprofile --norc -e -o pipefail /var/run/act/workflow/1] user= workdir=
|    Compiling hello_world v0.1.0 (/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo)
|      Running `/usr/share/rust/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc --crate-name hello_world --edition=2021 src/main.rs --error-format=json --json=diagnostic-rendered-ansi,artifacts,future-incompat --crate-type bin --emit=dep-info,link -C embed-bitcode=no -C debuginfo=2 -C metadata=63db71e2032dbcc4 -C extra-filename=-63db71e2032dbcc4 --out-dir /mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/deps -C incremental=/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/incremental -L dependency=/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/deps`
|     Finished dev [unoptimized + debuginfo] target(s) in 1.06s
[Rust/build-1]   ✅  Success - Main Build
[Rust/build-1] ⭐ Run Main Run tests
[Rust/build-1]   🐳  docker exec cmd=[bash --noprofile --norc -e -o pipefail /var/run/act/workflow/2] user= workdir=
|    Compiling hello_world v0.1.0 (/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo)
|      Running `/usr/share/rust/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc --crate-name hello_world --edition=2021 src/main.rs --error-format=json --json=diagnostic-rendered-ansi,artifacts,future-incompat --emit=dep-info,link -C embed-bitcode=no -C debuginfo=2 --test -C metadata=4b61e41928288fa3 -C extra-filename=-4b61e41928288fa3 --out-dir /mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/deps -C incremental=/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/incremental -L dependency=/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/deps`
|     Finished test [unoptimized + debuginfo] target(s) in 0.42s
|      Running `/mnt/c/Users/zhangchao/Source/github/zhangchao/github-action-demo/target/debug/deps/hello_world-4b61e41928288fa3`
|
| running 0 tests
|
| test result: ok. 0 passed; 0 failed; 0 ignored; 0 measured; 0 filtered out; finished in 0.00s
|
[Rust/build-1]   ✅  Success - Main Run tests
[Rust/build-1] Cleaning up container for job build
[Rust/build-1] 🏁  Job succeeded
```

When act is running, you can see a container is running, and the container will deleted after act exit.

```powershell
$ podman -c podman-machine-2 ps -a
CONTAINER ID  IMAGE                                    COMMAND     CREATED       STATUS       PORTS       NAMES
ea8c81ff30d9  ghcr.io/catthehacker/ubuntu:rust-latest              1 second ago  Up 1 second              act-Rust-build-1-35f44903c3fd3bd78b30863152e414926009860bcc195b646df625e978c11871
```

maybe you should add the podman identity key to ssh-agent first.

```powershell
ssh-add $HOME/.ssh/podman-machine-2
```

## podman on linux

Add `CONTAINER_HOST` env var by `export CONTAINER_HOST=unix://$(podman info --format '{{.Host.RemoteSocket.Path}}')`

```bash
$ act --container-daemon-socket $CONTAINER_HOST -P ubuntu-latest=catthehacker/ubuntu:rust-latest
[Rust/build-1] 🚀  Start image=catthehacker/ubuntu:rust-latest
[Rust/build-2] 🚧  Skipping unsupported platform -- Try running with `-P windows-latest=...`
[Rust/build-3] 🚧  Skipping unsupported platform -- Try running with `-P macos-latest=...`
[Rust/build-1]   🐳  docker pull image=catthehacker/ubuntu:rust-latest platform= username= forcePull=true
[Rust/build-1]   🐳  docker create image=catthehacker/ubuntu:rust-latest platform= entrypoint=["tail" "-f" "/dev/null"] cmd=[]
[Rust/build-1]   🐳  docker run image=catthehacker/ubuntu:rust-latest platform= entrypoint=["tail" "-f" "/dev/null"] cmd=[]
[Rust/build-1] 🧪  Matrix: map[os:ubuntu-latest]
[Rust/build-1] ⭐ Run Main actions/checkout@v3
[Rust/build-1]   🐳  docker cp src=/home/zhangchao/sources/github/zhangchao/github-action-demo/. dst=/home/zhangchao/sources/github/zhangchao/github-action-demo
[Rust/build-1]   ✅  Success - Main actions/checkout@v3
[Rust/build-1] ⭐ Run Main Build
[Rust/build-1]   🐳  docker exec cmd=[bash --noprofile --norc -e -o pipefail /var/run/act/workflow/1] user= workdir=
|    Compiling hello_world v0.1.0 (/home/zhangchao/sources/github/zhangchao/github-action-demo)
|      Running `/usr/share/rust/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc --crate-name hello_world --edition=2021 src/main.rs --error-format=json --json=diagnostic-rendered-ansi,artifacts,future-incompat --crate-type bin --emit=dep-info,link -C embed-bitcode=no -C debuginfo=2 -C metadata=63db71e2032dbcc4 -C extra-filename=-63db71e2032dbcc4 --out-dir /home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/deps -C incremental=/home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/incremental -L dependency=/home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/deps`
|     Finished dev [unoptimized + debuginfo] target(s) in 0.11s
[Rust/build-1]   ✅  Success - Main Build
[Rust/build-1] ⭐ Run Main Run tests
[Rust/build-1]   🐳  docker exec cmd=[bash --noprofile --norc -e -o pipefail /var/run/act/workflow/2] user= workdir=
|    Compiling hello_world v0.1.0 (/home/zhangchao/sources/github/zhangchao/github-action-demo)
|      Running `/usr/share/rust/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc --crate-name hello_world --edition=2021 src/main.rs --error-format=json --json=diagnostic-rendered-ansi,artifacts,future-incompat --emit=dep-info,link -C embed-bitcode=no -C debuginfo=2 --test -C metadata=4b61e41928288fa3 -C extra-filename=-4b61e41928288fa3 --out-dir /home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/deps -C incremental=/home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/incremental -L dependency=/home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/deps`
|     Finished test [unoptimized + debuginfo] target(s) in 0.11s
|      Running `/home/zhangchao/sources/github/zhangchao/github-action-demo/target/debug/deps/hello_world-4b61e41928288fa3`
| 
| running 0 tests
| 
| test result: ok. 0 passed; 0 failed; 0 ignored; 0 measured; 0 filtered out; finished in 0.00s
| 
[Rust/build-1]   ✅  Success - Main Run tests
[Rust/build-1] 🏁  Job succeeded
```

## use cake action

Get the `GitHubActions` object on ubuntu-latest

```json
{
  "IsRunningOnGitHubActions": true,
  "Environment": {
    "Home": "/home/runner",
    "Runner": {
      "Name": "GitHub Actions 10",
      "OS": "Linux",
      "Temp": "/home/runner/work/_temp",
      "ToolCache": "/opt/hostedtoolcache",
      "Workspace": "/home/runner/work/github-action-demo",
      "ImageOS": "ubuntu22",
      "ImageVersion": "20240721.1.0",
      "User": "runner",
      "Architecture": 2
    },
    "Workflow": {
      "Action": "__cake-build_cake-action",
      "ActionPath": null,
      "Actor": "zhangchaoza",
      "ApiUrl": "https://api.github.com",
      "BaseRef": "",
      "EventName": "workflow_dispatch",
      "EventPath": "/home/runner/work/_temp/_github_workflow/event.json",
      "GraphQLUrl": "https://api.github.com/graphql",
      "HeadRef": "",
      "Job": "print",
      "Ref": "refs/heads/main",
      "Repository": "zhangchaoza/github-action-demo",
      "RepositoryOwner": "zhangchaoza",
      "RunId": "10195649957",
      "RunNumber": 5,
      "ServerUrl": "https://github.com",
      "Sha": "48b5be64dfec2916d6882a55c947515e823a0a94",
      "Workflow": "cake workflow",
      "Workspace": "/home/runner/work/github-action-demo/github-action-demo",
      "Attempt": 1,
      "RefProtected": false,
      "RefName": "main",
      "RefType": 1
    },
    "PullRequest": {
      "IsPullRequest": false
    },
    "Runtime": {
      "IsRuntimeAvailable": true,
      "Token": ***,
      "Url": "https://pipelinesghubeus4.actions.githubusercontent.com/3yeEyfbM7ycsZ4BnfFviynrY0v0rwz0C8aWsqTYPqw2yPJGL4h/",
      "EnvPath": "/home/runner/work/_temp/_runner_file_commands/set_env_99f33bf6-4845-44f9-93cd-49ca705049c4",
      "OutputPath": "/home/runner/work/_temp/_runner_file_commands/set_output_99f33bf6-4845-44f9-93cd-49ca705049c4",
      "StepSummary": "/home/runner/work/_temp/_runner_file_commands/step_summary_99f33bf6-4845-44f9-93cd-49ca705049c4",
      "SystemPath": "/home/runner/work/_temp/_runner_file_commands/add_path_99f33bf6-4845-44f9-93cd-49ca705049c4"
    }
  },
  "Commands": {}
}
```

Get `GitVersion` object

```json
{
  "Major": 1,
  "Minor": 0,
  "Patch": 1,
  "PreReleaseTag": "9",
  "PreReleaseTagWithDash": "-9",
  "PreReleaseLabel": "",
  "PreReleaseLabelWithDash": "",
  "PreReleaseNumber": 9,
  "WeightedPreReleaseNumber": 55009,
  "BuildMetaData": null,
  "BuildMetaDataPadded": null,
  "FullBuildMetaData": "Branch.main.Sha.48b5be64dfec2916d6882a55c947515e823a0a94",
  "MajorMinorPatch": "1.0.1",
  "SemVer": "1.0.1-9",
  "LegacySemVer": null,
  "LegacySemVerPadded": null,
  "AssemblySemVer": "1.0.1.0",
  "AssemblySemFileVer": "1.0.1.0",
  "FullSemVer": "1.0.1-9",
  "InformationalVersion": "1.0.1-9+Branch.main.Sha.48b5be64dfec2916d6882a55c947515e823a0a94",
  "BranchName": "main",
  "EscapedBranchName": "main",
  "Sha": "48b5be64dfec2916d6882a55c947515e823a0a94",
  "ShortSha": "48b5be6",
  "NuGetVersionV2": null,
  "NuGetVersion": null,
  "NuGetPreReleaseTagV2": null,
  "NuGetPreReleaseTag": null,
  "VersionSourceSha": "2ab2cce3696aca056b58c14c8be25ef4017d76a0",
  "CommitsSinceVersionSource": 9,
  "CommitsSinceVersionSourcePadded": null,
  "UncommittedChanges": 0,
  "CommitDate": "2024-08-01"
}
```
