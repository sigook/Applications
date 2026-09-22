FROM nginx:stable-alpine
EXPOSE 80

COPY wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html

HEALTHCHECK --interval=30s --timeout=30s --retries=3 CMD curl --fail http://localhost:80 || exit

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]
