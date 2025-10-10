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

The Ingress defines the public domain and routes traffic to the `gatewayservice`.

There are **two ways** to apply the ingress configuration:

### 🟢 Option 1 – Apply from local file
If you have the YAML file in your project (for example in `k8s/ingress/gateway-ingress.yaml`):

```
kubectl apply -f k8s/ingress/gateway-ingress.yaml
```

This applies the ingress directly from your local repository.

---

### 🌐 Option 2 – Apply directly from GitHub
If the file is already pushed to your GitHub repository,  
you can apply it **directly from the raw URL** without downloading it:

```
kubectl apply -f https://raw.githubusercontent.com/JakubKopecky-dev/ECommerceApp/main/k8s/ingress/gateway-ingress.yaml
```

This downloads the YAML from GitHub and applies it straight to your AKS cluster.  
✅ Ideal for setup in Azure Cloud Shell.

---

### 🔍 Verify deployment:
```
kubectl get ingress -n ecommerceapp
kubectl describe ingress gateway-ingress -n ecommerceapp
```

You should see your domain listed under `ADDRESS` and `HOSTS`, e.g.:

```
ADDRESS: 9.163.223.240
HOSTS: ecommerce.9.163.223.240.nip.io
```

---

💡 **Tip:**  
Once this ingress is applied, your `GatewayService` will automatically be accessible through HTTPS  
as soon as CI/CD deploys the application pods.

---

### 💡 Tips

- To use a different domain, edit the `host` field in `gateway-ingress.yaml`.
- If the certificate isn’t issued, check logs:
  ```
  kubectl logs -n cert-manager -l app.kubernetes.io/component=controller
  ```
- For staging environments, you can create a test ClusterIssuer using the Let's Encrypt staging API (`staging.api.letsencrypt.org`).

---
