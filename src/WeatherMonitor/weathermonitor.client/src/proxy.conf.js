import { env } from "process";

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
  ? env.ASPNETCORE_URLS.split(";")[0]
  : "https://localhost:7035";

const PROXY_CONFIG = [
  {
    context: ['/api', '/settings', '/hub'],
    target,
    secure: false,
  },
];

export default PROXY_CONFIG;
