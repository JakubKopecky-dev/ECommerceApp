# Ingress Controller & Cert-Manager Setup Guide

This guide explains how to install and configure the **NGINX Ingress Controller** and **Cert-Manager** for your AKS cluster.
You only need to do this once — when you create the cluster for the first time (or after it’s deleted).

---

## 🧩 1️⃣ Install NGINX Ingress Controller

The NGINX Ingress Controller acts as the main entry point into the Kubernetes cluster (reverse proxy for all services).

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/cloud/deploy.yaml
```

Verify installation:

```
kubectl get pods -n ingress-nginx
kubectl get svc -n ingress-nginx
```

> In the service list, you should see a `LoadBalancer` with a public IP address.

---

## 🔐 2️⃣ Install Cert-Manager (Let's Encrypt)

Cert-Manager automatically manages SSL/TLS certificates (in our case using Let's Encrypt).

### Add the Helm repository:
```
helm repo add jetstack https://charts.jetstack.io
helm repo update
```

### Install Cert-Manager:
```
helm install cert-manager jetstack/cert-manager   --namespace cert-manager   --create-namespace   --version v1.15.3   --set installCRDs=true
```

### Verify installation:
```
kubectl get pods -n cert-manager
```

---

## 🧾 3️⃣ Create ClusterIssuer (Let's Encrypt)

The ClusterIssuer defines how Cert-Manager should request certificates from Let's Encrypt.

```
kubectl apply -f k8s/ingress/cert-manager-clusterissuer.yaml
```

Verify it was created successfully:
```
kubectl get clusterissuer
```

---

## 🌐 4️⃣ Deploy the Ingress

The Ingress defines the public domain and routes traffic to the gateway service.

```
kubectl apply -f k8s/ingress/gateway-ingress.yaml
```

Verify:
```
kubectl get ingress -n ecommerceapp
kubectl describe ingress gateway-ingress -n ecommerceapp
```

---

## ✅ 5️⃣ Test Access

Once the Ingress has an `ADDRESS` (IP), open your app in a browser:

```
https://ecommerce.<your-public-ip>.nip.io
```

> The Let's Encrypt certificate should be automatically issued within a few minutes.

---

### 💡 Tips

- To use a different domain, edit the `host` field in `gateway-ingress.yaml`.
- If the certificate isn’t issued, check logs:
  ```
  kubectl logs -n cert-manager -l app.kubernetes.io/component=controller
  ```
- For staging environments, you can create a test ClusterIssuer using the Let's Encrypt staging API (`staging.api.letsencrypt.org`).

---

