<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

 <xsl:template match="tasks">
	 <configuration version="1">
		 <tasks>
			 <xsl:apply-templates select="task" />
		 </tasks>
		 <xsl:apply-templates select="components" />
		 <xsl:apply-templates select="addins" />
	 </configuration>
 </xsl:template>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>