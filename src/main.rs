fn main() {
    println!("Hello, world! by {}", simple_fn());
}

#[cfg(target_os = "windows")]
fn simple_fn<'a>() -> &'a str {
    "Windows"
}

#[cfg(target_os = "linux")]
fn simple_fn<'a>() -> &'a str {
    "Linux"
}

#[cfg(target_os = "macos")]
fn simple_fn<'a>() -> &'a str {
    "MacOS"
}
