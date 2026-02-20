# ?? BLOCO 5.4 - CONNECTION MANAGER TEST PROCEDURES

**Projeto**: Quatto API Client - Configuration Wizard  
**Fase**: 3 (Testes)  
**Bloco**: 5.4 - Connection Manager Integration  
**Data de Teste**: ________________  
**Testador**: ________________  

---

## ?? OBJETIVO

Validar que o wizard se integra corretamente com o Connection Manager SSIS, carregando valores e sincronizando autenticação.

---

## ?? TEST OVERVIEW

```
???????????????????????????????????????????????????????????
? Teste                               ? Status   ? Tempo  ?
???????????????????????????????????????????????????????????
? 1. CM Carrega no ComboBox           ? ? _____ ? ___ min?
? 2. CM Integra com Wizard            ? ? _____ ? ___ min?
? 3. Autenticação & Sincronização     ? ? _____ ? ___ min?
???????????????????????????????????????????????????????????
? TOTAL                               ? ? _____ ? ___ min?
???????????????????????????????????????????????????????????

Status Legenda: ? PASSOU | ? FALHOU | ?? AVISO
```

---

## TESTE 1: CM CARREGA NO COMBOBOX

### Objetivo
Verificar que Connection Managers disponíveis aparecem na ComboBox.

### Pré-requisitos
```
[ ] SSDT aberto com package
[ ] 1-2 Connection Managers criados (Ex: OleDbConnectionManager)
[ ] Wizard aberto
[ ] ComboBox "Conexão" visível
```

### Procedimento

```
1. [ ] Abrir SSDT com um Integration Services Package
2. [ ] Criar 2-3 Connection Managers de teste:
   [ ] oledb_test1
   [ ] oledb_test2
   [ ] (opcional) sql_test
3. [ ] Abrir nosso wizard (direita ? Edit)
4. [ ] Observar ComboBox "Conexão" na aba General
5. [ ] Verificar lista de opções
```

### Resultado Esperado

```
[ ] ComboBox não está vazio
[ ] Lista mostra Connection Managers criados:
    [ ] oledb_test1 aparece
    [ ] oledb_test2 aparece
    [ ] (se criado) sql_test aparece
[ ] Pode selecionar um item
[ ] Sem exceptions na console
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

CMs encontrados:
_________________________________________________________________

Observações:
_________________________________________________________________
_________________________________________________________________

Issues:
[ ] Nenhum
[ ] Sim (listar abaixo)
```

---

## TESTE 2: CM INTEGRA COM WIZARD

### Objetivo
Verificar que selecionando um CM, o wizard se comporta corretamente.

### Pré-requisitos
```
[ ] TESTE 1 passou (CM carregou)
[ ] 2-3 CMs disponíveis no ComboBox
[ ] Wizard aberto
```

### Procedimento

```
1. [ ] Abrir ComboBox "Conexão"
2. [ ] Selecionar primeiro CM (oledb_test1)
3. [ ] Observar reação
4. [ ] Selecionar segundo CM (oledb_test2)
5. [ ] Observar reação
6. [ ] Voltar a vazio (selecionar índice -1 ou blank)
7. [ ] Observar reação
```

### Resultado Esperado

```
[ ] Ao selecionar CM:
    [ ] Sem delay excessivo
    [ ] Sem freeze da UI
    [ ] ComboBox atualiza corretamente
    
[ ] Outros controles não são afetados:
    [ ] Base URL: permanece igual
    [ ] Endpoint: permanece igual
    [ ] Outros: não mudam
    
[ ] Pode clicar OK com CM selecionado:
    [ ] Sem erro de validação
    [ ] Wizard fecha normalmente
    [ ] Package salva OK
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

CM selecionado: ___________________________
Performance: [ ] Rápido (<500ms)  [ ] Aceitável (500-2s)  [ ] Lento (>2s)

Outros controles afetados?
[ ] Nenhum (OK!)
[ ] Sim (listar): __________________________________________

Observações:
_________________________________________________________________
_________________________________________________________________

Issues:
[ ] Nenhum
[ ] Sim (listar abaixo)
```

---

## TESTE 3: AUTENTICAÇÃO & SINCRONIZAÇÃO

### Objetivo
Validar que CM e wizard sincronizam valores corretamente.

### Pré-requisitos
```
[ ] TESTE 2 passou
[ ] 1 CM selecionado no wizard
[ ] Wizard aberto
[ ] Connection Manager configurado com credenciais
```

### Procedimento

```
1. [ ] Selecionar um CM no ComboBox
2. [ ] Verificar se CM tem credenciais salvas
3. [ ] Configurar outros campos:
   [ ] Base URL: https://api.test.com
   [ ] Endpoint: /v1/data
   [ ] Page Size: 500
4. [ ] Clicar OK (salvar)
5. [ ] Fechar Package
6. [ ] Reabrir Package
7. [ ] Double-click no componente para abrir wizard
8. [ ] Verificar se valores persistem
```

### Resultado Esperado

```
[ ] CM selecionado persiste:
    [ ] Mesmo CM ainda selecionado após reabrir
    [ ] Nenhuma credencial perdida
    
[ ] Outros valores persistem:
    [ ] Base URL: https://api.test.com
    [ ] Endpoint: /v1/data
    [ ] Page Size: 500
    
[ ] Autenticação funciona:
    [ ] CM consegue conectar (se configurado)
    [ ] Sem timeout
    [ ] Sem erro de credenciais
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

CM selecionado persiste?
[ ] Sim (mesmo CM)
[ ] Não (mudou ou saiu vazio)
[ ] Parcial (algo diferente)

Valores persistem?
[ ] Base URL: [ ] SIM  [ ] NÃO
[ ] Endpoint: [ ] SIM  [ ] NÃO
[ ] Page Size: [ ] SIM  [ ] NÃO

Credenciais/Autenticação:
[ ] OK (conecta sem erro)
[ ] Aviso: _____________________________________________
[ ] Erro: _______________________________________________

Observações:
_________________________________________________________________
_________________________________________________________________

Issues:
[ ] Nenhum
[ ] Sim (listar abaixo)
```

---

## ?? RESUMO GERAL

```
???????????????????????????????????????????????????????????
? Teste                               ? Status   ? Tempo  ?
???????????????????????????????????????????????????????????
? 1. CM Carrega no ComboBox           ? [ ] ____ ? ___ min?
? 2. CM Integra com Wizard            ? [ ] ____ ? ___ min?
? 3. Autenticação & Sincronização     ? [ ] ____ ? ___ min?
???????????????????????????????????????????????????????????
? TOTAL                               ? [ ] ____ ? ___ min?
???????????????????????????????????????????????????????????

Status Legenda: ? PASSOU | ? FALHOU | ?? AVISO
```

---

## ? DECISÃO FINAL

```
Todos os testes passaram?
[ ] SIM ? Ir para BLOCO 5.5
[ ] COM RESSALVAS ? Documentar em ISSUES_LOG
[ ] NÃO ? Ver Issues e rejeitar

Conformidade:
?? Target: 95%+
?? Obtido: ___%
?? Status: [ ] ALCANÇADO  [ ] NÃO ALCANÇADO
```

---

## ?? NOTAS

```
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
```

---

## ?? ASSINATURA

```
Testador: ___________________________    Data: ______________
Revisor: ___________________________    Data: ______________
```

---

**Versão**: 3.0.4-BLOCO-5.4  
**Status**: ?? PRONTO PARA USAR

