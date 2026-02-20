# Security Policy

## Reporting a Vulnerability

**⚠️ IMPORTANTE:** Se você descobrir uma vulnerabilidade de segurança, **NÃO** crie uma issue pública. Isso poderia expor o problema para usuários maliciosos.

### Como Reportar com Segurança

1. **Vá para:** [Security Advisories](https://github.com/ertonjm/QuattoAPIClient/security/advisories/new)
2. **Ou envie email para:** security@quatto.com.br
3. **Inclua:**
   - Descrição detalhada da vulnerabilidade
   - Passos para reproduzir
   - Possível impacto
   - Sua informação de contato

### Nossas Responsabilidades

- ✅ Responderemos em até **48 horas**
- ✅ Investigaremos a vulnerabilidade
- ✅ Trabalharemos em um patch discreto
- ✅ Creditaremos o descobridor (a menos que prefira anonimato)
- ✅ Criaremos um Security Advisory antes de publicar fix

---

## Processo de Divulgação Responsável

### Timeline Esperada

1. **Dia 1:** Você reporta a vulnerabilidade
2. **Dia 2:** Confirmamos recebimento
3. **Dia 3-7:** Investigação e desenvolvimento do patch
4. **Dia 7-14:** Testes e validação
5. **Dia 15:** Patch é lançado
6. **Dia 15:** Security Advisory é publicado

---

## Suportamos

### Versões Atualmente Suportadas

| Versão | Suporte | Data de Fim |
|--------|---------|-------------|
| 1.x.x | ✅ Ativo | TBD |
| 0.x.x | ❌ Fim | 2025-06-20 |

Apenas a versão mais recente recebe patches de segurança.

### Ambientes Suportados

- ✅ SQL Server 2022 (v17.x, v18.x)
- ✅ SQL Server 2019 (v16.x)
- ✅ Visual Studio 2022 (v17.x)
- ✅ .NET Framework 4.7.2 ou superior
- ✅ Windows Server 2019+
- ✅ Windows 10/11

---

## Tipos de Vulnerabilidades que Aceitamos

### 🔴 Crítico
- SQL Injection
- Remote Code Execution (RCE)
- Authentication Bypass
- Privilege Escalation
- Data Exposure

### 🟠 Alto
- Cross-Site Request Forgery (CSRF)
- Insecure Deserialization
- Weak Cryptography
- XXE (XML External Entity)
- Path Traversal

### 🟡 Médio
- Information Disclosure
- Denial of Service (DoS)
- Insecure Direct Object Reference (IDOR)
- Missing Access Control
- Insufficient Logging

### 🟢 Baixo
- Weak Password Policy
- Missing Security Headers
- Outdated Dependencies
- Typos em mensagens de segurança

---

## Melhorias de Segurança Planejadas

### Implementadas
- ✅ `.gitattributes` para proteção de binários
- ✅ `.gitignore` para não commitar secrets
- ✅ Code of Conduct para comunidade segura
- ✅ Security policy transparente

### Planejadas
- 🔄 GitHub Advanced Security scanning
- 🔄 Dependabot para atualizar dependências
- 🔄 SAST (Static Application Security Testing)
- 🔄 DAST (Dynamic Application Security Testing)
- 🔄 Secret scanning
- 🔄 Supply chain security checks

---

## Boas Práticas de Segurança

### Para Usuários
1. **Mantenha atualizado** - Sempre use a versão mais recente
2. **Validate inputs** - Nunca confie em dados do usuário
3. **Use HTTPS** - Sempre comunique com APIs via HTTPS
4. **Proteja tokens** - Nunca exponha GitHub tokens ou API keys
5. **Audit logs** - Monitore atividades suspeitas

### Para Desenvolvedores
1. **Code review obrigatório** - Mínimo 1 aprovação
2. **Testes de segurança** - Teste casos de segurança
3. **Dependências atualizadas** - Use versões seguras
4. **Secrets management** - Use .gitignore e variáveis de ambiente
5. **Logging seguro** - Não logue informações sensíveis

---

## Ferramentas de Segurança Usadas

- **GitHub Advanced Security** - Scanning automático
- **Dependabot** - Atualizações de dependências
- **SonarQube** (planejado) - Análise de código
- **OWASP ZAP** (planejado) - Teste de segurança dinâmica
- **Snyk** (planejado) - Vulnerabilidade scanning

---

## Links de Referência

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE/SANS Top 25](https://cwe.mitre.org/top25/)
- [CVSS Calculator](https://www.first.org/cvss/calculator/3.1)
- [GitHub Security Best Practices](https://docs.github.com/en/code-security)
- [Microsoft Security Best Practices](https://learn.microsoft.com/dotnet/standard/security/)

---

## Contatos de Segurança

| Meio | Contato |
|------|---------|
| 📧 **Email** | security@quatto.com.br |
| 🔐 **GitHub** | [Security Advisories](https://github.com/ertonjm/QuattoAPIClient/security/advisories) |
| 💬 **Discussions** | Privado quando disponível |

---

## Agradecimentos

Agradecemos a todos os pesquisadores de segurança que reportam vulnerabilidades responsavelmente. Você ajuda a manter nossa comunidade segura! 🙏

---

## Histórico de Vulnerabilidades

Nenhuma vulnerabilidade reportada até agora. ✅

Quando houver, será listada aqui com:
- Data de descoberta
- Severidade (CVSS)
- Afetados (versões)
- Status (Patch disponível)
- Data de divulgação pública

---

**Última atualização:** 2025-02-20  
**Versão:** 1.0.0  
**Mantido por:** @ertonjm

