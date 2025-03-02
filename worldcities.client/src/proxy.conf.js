const PROXY_CONFIG = [
  {
    context: [
      "/api/1.1",
    ],
    target: "localhost:8088",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
