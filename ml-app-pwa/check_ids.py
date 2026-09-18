with open(r"C:\Users\BrunoTaraborrelli\Desktop\Proyecto UTN\ml-app-pwa\index.html", "r", encoding="utf-8") as f:
    c = f.read()

import re
tags = re.findall(r'document\.getElementById\(["\'](.*?)["\']\)', c)
print("TOTAL GETELEMENTBYID:", len(tags))
unique_ids = set(tags)
for i in unique_ids:
    if f'id="{i}"' not in c and f"id='{i}'" not in c:
        print("ALERTA ID NO ENCONTRADO EN HTML:", i)